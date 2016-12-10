# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('pools', '0003_auto_20150428_1433'),
    ]

    operations = [
        migrations.RenameModel(
            old_name='STH',
            new_name='API',
        ),
    ]
