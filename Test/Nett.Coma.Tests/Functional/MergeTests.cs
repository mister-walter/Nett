﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentAssertions;
using Nett.Coma.Tests.TestData;
using Nett.Tests.Util;
using Xunit;

namespace Nett.Coma.Tests.Functional
{
    [ExcludeFromCodeCoverage]
    public sealed class MergeTests : TestsBase
    {
        private const string FuncLoadMergedConfig = "Load Merged Config";

        const string Config1 = "IntValue = 1";
        const string Config2 = "StringValue = 'test'";
        const string Config1A = "IntValue = 2";

        [FFact(FuncLoadMergedConfig, "When multiple sources used, merges those into one in process config object.")]
        public void LoadMergedConfig_MergesSourcesIntoOneInProcessConfig()
        {
            string f1 = "config1".TestRunUniqueName() + Toml.FileExtension;
            string f2 = "config2".TestRunUniqueName() + Toml.FileExtension;

            try
            {
                // Arrange
                File.WriteAllText(f1, Config1);
                File.WriteAllText(f2, Config2);

                // Act
                var c = Config.CreateAs()
                    .MappedToType(() => new SingleLevelConfig())
                    .StoredAs(store =>
                        store.File(f1).MergeWith(
                            store.File(f2)))
                    .Initialize();

                // Assert
                c.Get(cfg => cfg.IntValue).Should().Be(1);
                c.Get(cfg => cfg.StringValue).Should().Be("test");
            }
            finally
            {
                TryDeleteFile(f1);
                TryDeleteFile(f2);
            }
        }

        [FFact(FuncLoadMergedConfig, "When same setting in both files the 'more local' setting will overwrite the 'more global' value")]
        public void LoadMergedConfig_LocalSettingOverwritesMoreGlobalSetting()
        {
            using (var global = TestFileName.Create("global", Toml.FileExtension))
            using (var local = TestFileName.Create("local", Toml.FileExtension))
            {
                // Arrange
                File.WriteAllText(global, Config1);
                File.WriteAllText(local, Config1A);

                // Act
                var c = Config.CreateAs()
                    .MappedToType(() => new SingleLevelConfig())
                    .StoredAs(store =>
                        store.File(global).MergeWith(
                            store.File(local)))
                    .Initialize();

                // Assert
                c.Get(r => r.IntValue).Equals(2);
            }
        }

        [FFact(FuncLoadMergedConfig, "When values defined in both sources, the values from the 'successor' source overwrite values of the predecessor source")]
        public void LoadMergedConfig_SucessorOverwritesPredecessorValues()
        {
            string f1 = "config1".TestRunUniqueName() + Toml.FileExtension;
            string f2 = "config2".TestRunUniqueName() + Toml.FileExtension;

            try
            {
                // Arrange
                const string Pre = @"
IntValue = 1
StringValue = 'pre'";
                const string Succ = @"
StringValue = 'succ'";

                File.WriteAllText(f1, Pre);
                File.WriteAllText(f2, Succ);

                // Act
                var c = Config.CreateAs()
                    .MappedToType(() => new SingleLevelConfig())
                    .StoredAs(store =>
                        store.File(f1).MergeWith(
                            store.File(f2)))
                    .Initialize();

                // Assert
                c.Get(cfg => cfg.IntValue).Should().Be(1);
                c.Get(cfg => cfg.StringValue).Should().Be("succ");
            }
            finally
            {
                TryDeleteFile(f1);
                TryDeleteFile(f2);
            }
        }

        [FFact(FuncLoadMergedConfig, "Initial Git multi file config is merged into single config object correctly")]
        public void Merge_WhenUsingGitScenario_MergesConfigCorrectly()
        {
            using (var s = GitScenario.Setup(nameof(Merge_WhenUsingGitScenario_MergesConfigCorrectly)))
            {
                // Act
                var cfg = Config.CreateAs()
                    .MappedToType(() => new GitScenario.GitConfig())
                    .StoredAs(store =>
                        store.File(s.SystemFile).MergeWith(
                            store.File(s.UserFile).MergeWith(
                               store.File(s.RepoFile))))
                    .Initialize();

                // Assert
                var x = cfg.Unmanaged();
                Assert.True(cfg.Unmanaged().Equals(GitScenario.MergedDefault));
            }

        }
    }
}
